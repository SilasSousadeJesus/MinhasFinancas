"use client";

import * as React from "react";
import Link from "next/link";

import { cn } from "@/lib/utils";
import {
  NavigationMenu,
  NavigationMenuContent,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
  NavigationMenuTrigger,
  navigationMenuTriggerStyle,
} from "@/components/ui/navigation-menu";
import {
  CreditCardIcon,
  DownloadIcon,
  FilesIcon,
  LayoutDashboardIcon,
  SettingsIcon,
} from "../Icons/Icons";
import { useEffect, useState } from "react";

const components: { title: string; href: string; description: string }[] = [
  {
    title: "Alert Dialog",
    href: "/docs/primitives/alert-dialog",
    description:
      "A modal dialog that interrupts the user with important content and expects a response.",
  },
  {
    title: "Hover Card",
    href: "/docs/primitives/hover-card",
    description:
      "For sighted users to preview content available behind a link.",
  },
  {
    title: "Progress",
    href: "/docs/primitives/progress",
    description:
      "Displays an indicator showing the completion progress of a task, typically displayed as a progress bar.",
  },
  {
    title: "Scroll-area",
    href: "/docs/primitives/scroll-area",
    description: "Visually or semantically separates content.",
  },
  {
    title: "Tabs",
    href: "/docs/primitives/tabs",
    description:
      "A set of layered sections of content—known as tab panels—that are displayed one at a time.",
  },
  {
    title: "Tooltip",
    href: "/docs/primitives/tooltip",
    description:
      "A popup that displays information related to an element when the element receives keyboard focus or the mouse hovers over it.",
  },
];

export function MenuNavegacao() {
  const [isSidebarExpanded, setIsSidebarExpanded] = useState(false);
  const [windowWidth, setWindowWidth] = useState(0);

  useEffect(() => {
    const handleResize = () => {
      setWindowWidth(window.innerWidth);
      setIsSidebarExpanded(window.innerWidth >= 1024);
    };
    window.addEventListener("resize", handleResize);
    handleResize();
    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, []);

  return (
    <NavigationMenu className="">
      <NavigationMenuList className="flex flex-col ">
        <NavigationMenuItem className="w-full ">
          <Link href="/docs" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                " justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <LayoutDashboardIcon
                className={`w-5 h-5 `}
              />
              <span className={`space-x-2 text-lg ${!isSidebarExpanded && "hidden"}`}>Dashboard</span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full" >
          <Link href="/docs" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                " justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <CreditCardIcon className="w-5 h-5" />
              <span className={`space-x-2 text-lg ${!isSidebarExpanded && "hidden"}`}>Contas e Cartões</span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <Link href="/docs" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                " justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <DownloadIcon className="w-5 h-5" />
              <span className={`space-x-2 text-lg ${!isSidebarExpanded && "hidden"}`}>Lançamentos</span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <NavigationMenuTrigger className=" justify-center space-x-2 text-lg ">
            {" "}
            <FilesIcon className="w-5 h-5" />
            <span className={`space-x-2 text-lg ${!isSidebarExpanded && "hidden"}`}>Relatórios</span>
          </NavigationMenuTrigger>
          <NavigationMenuContent>
            <ul className="grid w-[400px] gap-3 p-4 md:w-[500px] md:grid-cols-2 lg:w-[600px] ">
              {components.map((component) => (
                <ListItem
                  key={component.title}
                  title={component.title}
                  href={component.href}
                >
                  {component.description}
                </ListItem>
              ))}
            </ul>
          </NavigationMenuContent>
        </NavigationMenuItem>

        <NavigationMenuItem className="w-full">
          <Link href="/docs" legacyBehavior passHref>
            <NavigationMenuLink
              className={cn(
                " justify-center space-x-2 text-lg",
                navigationMenuTriggerStyle()
              )}
            >
              <SettingsIcon className="w-5 h-5" />
              <span className={`space-x-2 text-lg ${!isSidebarExpanded && "hidden"}`}>Configurações</span>
            </NavigationMenuLink>
          </Link>
        </NavigationMenuItem>
      </NavigationMenuList>
    </NavigationMenu>
  );
}

const ListItem = React.forwardRef<
  React.ElementRef<"a">,
  React.ComponentPropsWithoutRef<"a">
>(({ className, title, children, ...props }, ref) => {
  return (
    <li>
      <NavigationMenuLink asChild>
        <a
          ref={ref}
          className={cn(
            "block select-none space-y-1 rounded-md p-3 leading-none no-underline outline-none transition-colors hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground",
            className
          )}
          {...props}
        >
          <div className="text-sm font-medium leading-none">{title}</div>
          <p className="line-clamp-2 text-sm leading-snug text-muted-foreground">
            {children}
          </p>
        </a>
      </NavigationMenuLink>
    </li>
  );
});
ListItem.displayName = "ListItem";
