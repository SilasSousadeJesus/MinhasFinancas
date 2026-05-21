"use client";

import { ProtectedRoute } from "@/components/auth/ProtectedRoute";

export default function AuthenticatedLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return <ProtectedRoute>{children}</ProtectedRoute>;
}
