"use client";

import { Moon, SunMedium } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useBibleStore } from "@/stores/bible-store";

export function ThemeToggle() {
  const theme = useBibleStore((state) => state.theme);
  const toggleTheme = useBibleStore((state) => state.toggleTheme);

  return (
    <Button
      size="icon"
      variant="outline"
      aria-label="Alternar tema claro ou escuro"
      onClick={toggleTheme}
    >
      {theme === "dark" ? <SunMedium className="size-4" /> : <Moon className="size-4" />}
    </Button>
  );
}
