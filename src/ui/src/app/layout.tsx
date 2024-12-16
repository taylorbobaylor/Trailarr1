import { useState, useEffect } from "react"; import PageHeader from "@/components/layout/PageHeader/PageHeader"; import PageSidebar from "@/components/layout/PageSidebar/PageSidebar"; export default function RootLayout({ children }: { children: React.ReactNode }) { const [isSmallScreen, setIsSmallScreen] = useState(false); const [isSidebarVisible, setIsSidebarVisible] = useState(true); useEffect(() => { const checkScreenSize = () => { setIsSmallScreen(window.innerWidth < 768); }; checkScreenSize(); window.addEventListener("resize", checkScreenSize); return () => window.removeEventListener("resize", checkScreenSize); }, []); const handleSidebarToggle = () => { setIsSidebarVisible(!isSidebarVisible); }; return ( <div className="min-h-screen bg-[var(--themeDarkColor)]"> <PageHeader onSidebarToggle={handleSidebarToggle} isSmallScreen={isSmallScreen} /> <PageSidebar isVisible={isSidebarVisible} onClose={() => setIsSidebarVisible(false)} /> <main className={`transition-all duration-200 ${isSidebarVisible ? "md:ml-64" : ""} p-4`}> {children} </main> </div> ); }
