import Link from "next/link";
import { ChevronRight } from "lucide-react";
import { cn } from "@/lib/utils";
import type { BreadcrumbItem } from "@/lib/seo";

export function SeoBreadcrumbs({ items }: { items: BreadcrumbItem[] }) {
  return (
    <nav aria-label="Breadcrumb" className="text-sm text-muted-foreground" itemScope itemType="https://schema.org/BreadcrumbList">
      <ol className="flex flex-wrap items-center gap-1">
        {items.map((item, index) => (
          <li
            key={item.href}
            itemProp="itemListElement"
            itemScope
            itemType="https://schema.org/ListItem"
            className="flex items-center gap-1"
          >
            <Link href={item.href} itemProp="item" className={cn("inline-flex items-center text-muted-foreground hover:text-primary")}> 
              <span itemProp="name">{item.label}</span>
            </Link>
            <meta itemProp="position" content={`${index + 1}`} />
            {index < items.length - 1 ? <ChevronRight className="h-4 w-4" /> : null}
          </li>
        ))}
      </ol>
    </nav>
  );
}
