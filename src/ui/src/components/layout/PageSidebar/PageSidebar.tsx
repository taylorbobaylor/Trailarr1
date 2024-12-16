"use client";

import { X } from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";

interface PageSidebarProps {
  isVisible: boolean;
  onClose: () => void;
}

export default function PageSidebar({ isVisible, onClose }: PageSidebarProps) {
  const pathname = usePathname();
  const navItems = [
    { href: "/", label: "Movies", icon: "📽️" },
    { href: "/settings", label: "Settings", icon: "⚙️" },
  ];

  return (
    <>
      {/* Mobile overlay */}
      {isVisible && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 z-30 md:hidden"
          onClick={onClose}
          aria-hidden="true"
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed top-16 left-0 bottom-0 w-64 bg-theme-dark border-r border-theme-dark transform transition-transform duration-200 z-40 ${
          isVisible ? "translate-x-0" : "-translate-x-full"
        } md:translate-x-0`}
      >
        <nav className="p-4">
          <div className="flex flex-col space-y-2">
            {navItems.map((item) => (
              <Link
                key={item.href}
                href={item.href}
                onClick={() => {
                  if (window.innerWidth < 768) onClose();
                }}
                className={`flex items-center px-4 py-2 rounded-lg transition-colors ${
                  pathname === item.href
                    ? "bg-theme-blue text-white"
                    : "text-theme-light hover:bg-theme-blue hover:bg-opacity-20"
                }`}
              >
                <span className="mr-3">{item.icon}</span>
                {item.label}
              </Link>
            ))}
          </div>
        </nav>

        {/* Mobile close button */}
        <button
          onClick={onClose}
          className="md:hidden absolute top-2 right-2 p-2 text-theme-light hover:text-white"
          aria-label="Close sidebar"
        >
          <X className="w-6 h-6" />
        </button>
      </aside>
    </>
  );
}
