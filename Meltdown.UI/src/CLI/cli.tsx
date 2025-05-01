import React, { JSX, useEffect } from "react";

import { Box, KeyMap, Text, Viewport, render, useKeymap } from "tuir";
import { patchConsole } from "./utils/console-utils.js";
import AppSimple from "./App.Simple.js";
import AppDefault from "./App.Default.js";
import { progressEmitter } from "./utils/commands.js";

export function cli() {
  patchConsole();

  render(<Root />, {
    patchConsole: true,
  });
}

function Root() {
  const variants = ["simple", "default"];
  const [variant, setVariant] = React.useState("simple");

  const keyMap = {
    quit: { input: "q" },
    variant: { input: "v" },
  } satisfies KeyMap;
  const { useEvent } = useKeymap(keyMap);

  useEvent("quit", () => {
    process.exit(0);
  });
  useEvent("variant", () => {
    const variantIndex = variants.indexOf(variant);
    const nextVariantIndex = (variantIndex + 1) % variants.length;
    setVariant(variants[nextVariantIndex]);
  });

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

  const app = chooseApp(variant, variants);

  return app;
}

function chooseApp(variant: string, knownVariants: string[]): JSX.Element {
  const isUnknownVariant = !knownVariants.includes(variant);
  return (
    <>
      <AppSimple display={variant == "simple" ? "flex" : "none"} />
      <AppDefault display={variant == "default" ? "flex" : "none"} />
      <Viewport display={isUnknownVariant ? "flex" : "none"} flexDirection="column"><Text>Unknown variant {variant}</Text></Viewport>
    </>
  );
}

export { commandEmitter } from "./utils/commands.js";
export { progressEmitter } from "./utils/commands.js";
