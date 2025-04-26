import { Link } from "@heroui/link";
import { Navbar } from "../components/navbar";
import { useStore } from "../stores/rootStore";
import { VersionStatus } from "../components/VersionStatus";


export default function DefaultLayout({
  children,
}: {
  children: React.ReactNode;
}) {

  const rootStore = useStore()
  return (
    <div className="relative flex flex-col h-screen">
      {rootStore.authStore.isAuthicated && <Navbar />}
      <main className="container mx-auto max-w-7xl px-6 flex-grow pt-16">
        {children}
      </main>
      <footer className="w-full flex items-center justify-center py-3">
        <Link
          isExternal
          className="flex items-center gap-1 text-current"
          href="https://github.com/tgiachi/orion"
          title="orionirc-server homepage"
        >
          <span className="text-default-600">Orion</span>
          <p className="text-primary">IRC Server</p>
          <VersionStatus />
        </Link>
      </footer>
    </div>
  );
}
