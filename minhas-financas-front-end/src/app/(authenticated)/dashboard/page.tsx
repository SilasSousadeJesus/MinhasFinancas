"use client";

import { PainelDashboard } from "@/components/dashboard/dashboard";
import { Sidebar } from "@/components/Sidebar/Sidebar";

export default function Dashboard() {
  return (
    <div className="flex flex-row">
      <Sidebar />
      <PainelDashboard/>
    </div>
  );
}
