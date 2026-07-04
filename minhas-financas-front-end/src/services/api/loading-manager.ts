export interface LoadingStateSnapshot {
  activeRequests: number;
  isVisible: boolean;
  message: string | null;
}

export interface LoadingRequestOptions {
  enabled?: boolean;
  message?: string;
}

type LoadingListener = (snapshot: LoadingStateSnapshot) => void;

const SHOW_DELAY_MS = 180;
const MIN_VISIBLE_MS = 300;

let activeRequests = 0;
let isVisible = false;
let visibleSince = 0;
let currentMessage: string | null = null;
let showTimer: ReturnType<typeof setTimeout> | null = null;
let hideTimer: ReturnType<typeof setTimeout> | null = null;

const listeners = new Set<LoadingListener>();

function emit() {
  const snapshot = getLoadingSnapshot();
  listeners.forEach((listener) => listener(snapshot));
}

function clearShowTimer() {
  if (!showTimer) {
    return;
  }

  clearTimeout(showTimer);
  showTimer = null;
}

function clearHideTimer() {
  if (!hideTimer) {
    return;
  }

  clearTimeout(hideTimer);
  hideTimer = null;
}

function showLoading() {
  clearShowTimer();

  if (isVisible || activeRequests <= 0) {
    return;
  }

  isVisible = true;
  visibleSince = Date.now();
  emit();
}

function hideLoading() {
  clearHideTimer();

  if (!isVisible) {
    currentMessage = null;
    emit();
    return;
  }

  isVisible = false;
  visibleSince = 0;
  currentMessage = null;
  emit();
}

function scheduleShow() {
  if (showTimer || isVisible || activeRequests <= 0) {
    return;
  }

  showTimer = setTimeout(() => {
    showLoading();
  }, SHOW_DELAY_MS);
}

function scheduleHide() {
  clearShowTimer();

  if (!isVisible) {
    currentMessage = null;
    emit();
    return;
  }

  const elapsed = Date.now() - visibleSince;
  const remaining = Math.max(MIN_VISIBLE_MS - elapsed, 0);

  if (remaining === 0) {
    hideLoading();
    return;
  }

  if (hideTimer) {
    return;
  }

  hideTimer = setTimeout(() => {
    hideLoading();
  }, remaining);
}

export function getLoadingSnapshot(): LoadingStateSnapshot {
  return {
    activeRequests,
    isVisible,
    message: currentMessage,
  };
}

export function subscribeToLoading(listener: LoadingListener) {
  listeners.add(listener);
  listener(getLoadingSnapshot());

  return () => {
    listeners.delete(listener);
  };
}

export function startGlobalLoading(options?: LoadingRequestOptions) {
  const enabled = options?.enabled ?? true;

  if (!enabled) {
    return () => {};
  }

  activeRequests += 1;
  currentMessage = options?.message ?? currentMessage;

  clearHideTimer();
  scheduleShow();
  emit();

  let finalized = false;

  return () => {
    if (finalized) {
      return;
    }

    finalized = true;
    activeRequests = Math.max(0, activeRequests - 1);

    if (activeRequests === 0) {
      scheduleHide();
    } else {
      emit();
    }
  };
}

