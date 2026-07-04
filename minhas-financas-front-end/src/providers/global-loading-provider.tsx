"use client";

import { ReactNode, createContext, useContext, useEffect, useMemo, useState } from "react";
import { GlobalLoading } from "@/components/GlobalLoading";
import {
  LoadingStateSnapshot,
  getLoadingSnapshot,
  subscribeToLoading,
} from "@/services/api/loading-manager";

const GlobalLoadingContext = createContext<LoadingStateSnapshot | undefined>(undefined);

export function GlobalLoadingProvider({ children }: { children: ReactNode }) {
  const [snapshot, setSnapshot] = useState<LoadingStateSnapshot>(() => getLoadingSnapshot());

  useEffect(() => {
    return subscribeToLoading(setSnapshot);
  }, []);

  const value = useMemo(() => snapshot, [snapshot]);

  return (
    <GlobalLoadingContext.Provider value={value}>
      {children}
      <GlobalLoading visible={snapshot.isVisible} message={snapshot.message} />
    </GlobalLoadingContext.Provider>
  );
}

export function useGlobalLoadingState() {
  const context = useContext(GlobalLoadingContext);

  if (!context) {
    throw new Error("useGlobalLoadingState deve ser usado dentro de GlobalLoadingProvider.");
  }

  return context;
}

