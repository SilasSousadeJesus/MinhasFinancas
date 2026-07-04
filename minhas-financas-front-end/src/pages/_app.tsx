import type { AppProps } from "next/app";
import "@/app/globals.css";
import { ThemeProvider } from "@/providers/theme-provider";
import { AuthProvider } from "@/providers/auth-provider";
import { GlobalLoadingProvider } from "@/providers/global-loading-provider";

export default function App({ Component, pageProps }: AppProps) {
  return (
    <ThemeProvider
      attribute="class"
      defaultTheme="system"
      enableSystem
      disableTransitionOnChange
    >
      <GlobalLoadingProvider>
        <AuthProvider>
          <Component {...pageProps} />
        </AuthProvider>
      </GlobalLoadingProvider>
    </ThemeProvider>
  );
}
