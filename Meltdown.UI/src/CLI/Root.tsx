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
} from "./tuir.js";
import {
  PageIndicator,
  progressEmitter
} from "./ink-components.js";
import { MasterDetail } from "./Pages/MasterDetail.js";
import { SimpleLog } from "./Pages/Simple.js";

type PagesReturn = ReturnType<typeof usePages>;
type PagesControl = PagesReturn["control"];

export function Root() {
  const variants = ["Simple", "MasterDetail"];
  const [variant, setVariant] = React.useState("simple");

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
          setVariant(args[0]);
        }
      }
    );
  }, [progressEmitter]);

  return (
    <Viewport flexDirection="column">
      <Pages pageView={pageView}>
        <SimpleLog />
        <MasterDetail />
      </Pages>
      <PageIndicator
        pageNames={variants}
        currentPage={pageControl.currentPage}
      />
      <Box marginTop={-1}>
        <Text>⌨️ </Text>
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
    process.exit(0);
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