"use client";

import { Menu } from "lucide-react";

interface PageHeaderProps {
  onSidebarToggle: () => void;
}

export default function PageHeader({ onSidebarToggle }: PageHeaderProps) {
  return (
    <header className="fixed top-0 left-0 right-0 h-16 bg-theme-dark border-b border-theme-dark z-50">
      <div className="flex items-center h-full px-4">
        <button
          onClick={onSidebarToggle}
          className="p-2 hover:bg-theme-blue hover:bg-opacity-20 rounded-lg transition-colors md:hidden"
          aria-label="Toggle sidebar"
        >
          <Menu className="w-6 h-6 text-theme-light" />
        </button>
        <div className="ml-4">
          <h1 className="text-xl font-semibold text-theme-light">Trailarr</h1>
        </div>
      </div>
    </header>
  );
}
