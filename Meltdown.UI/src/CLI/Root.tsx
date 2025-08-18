import React from "react";
import { useEffect } from "react";
import {
  Box,
  KeyMap,
  Pages,
  StdinState,
  Text,
  useKeymap,
  usePages,
  Viewport,
} from "./dependencies/tuir.js";
import { PageIndicator, progressEmitter, GradientText } from "./dependencies/ink-components.js";
import { MasterDetail } from "./Pages/MasterDetail.js";
import { SimpleLog } from "./Pages/Simple.js";
import { ProgressDemo } from "./Pages/ProgressDemo.js";

type PagesReturn = ReturnType<typeof usePages>;
type PagesControl = PagesReturn["control"];

const VARIANTS = ["simple", "master-detail", "treeview", "progress"] as const;
type RootVariant = (typeof VARIANTS)[number];

export function Root({
  init,
  variant: initialVariant,
}: { init?: () => void; variant?: RootVariant } = {}) {
  initialVariant ??= "treeview";
  const variants = VARIANTS as unknown as string[];
  const [variant, setVariant] = React.useState(initialVariant);

  const { pageView, control: pageControl } = usePages(variants.length);

  usePageNavigation(variants, pageControl);
  useEffect(() => {
    const index = variants.indexOf(variant);
    if (index === -1) {
      console.warn("Invalid variant selected:", variant);
      return;
    }
    pageControl.goToPage(index);
  }, [variant]);

  useEffect(() => {
    progressEmitter.on(
      "command",
      (command: string, path: string, args: string[]) => {
        if (command === "setVariant") {
          setVariant(args[0] as RootVariant);
        }
      }
    );
  }, [progressEmitter]);

  useEffect(() => {
    if (init) {
      init();
    }
  }, []);

  return (
    <Viewport flexDirection="column">
      <Pages pageView={pageView}>
        <SimpleLog />
        <MasterDetail variant="listView" />
        <MasterDetail variant="treeView" />
        <ProgressDemo />
      </Pages>
      <PageIndicator
        pageNames={variants}
        currentPage={pageControl.currentPage}
      />
      <Box marginTop={-1}>
        <GradientText startColor="#00ff00" endColor="#0000ff">[🛰️{process.pid}]</GradientText>
        <Text> ⌨️ </Text>
        <StdinState
          showEvents={true}
          showRegister={true}
          eventStyles={{
            color: "green",
          }}
          registerStyles={{
            color: "blue",
          }}
          width={25}
        />
      </Box>
    </Viewport>
  );
}

function usePageNavigation(pageNames: string[], pageControl: PagesControl) {
  const pagesKeymap = {
    goToPage: pageNames.map((_, idx) => ({
      input: `${idx + 1}`,
    })),
    quit: { input: "q" },
    //command: { input: ":", key: "f1" },
  } satisfies KeyMap;
  const { useEvent } = useKeymap(pagesKeymap);

  useEvent("quit", () => {
    //process.exit(0);
  });

  useEvent("goToPage", (char: string) => {
    const pageIndex = Number(char);

    if (
      !Number.isNaN(pageIndex) &&
      pageIndex > 0 &&
      pageIndex <= pageNames.length
    ) {
      pageControl.goToPage(pageIndex - 1);
    }
  });
}
