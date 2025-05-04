import React from "react";

import { render } from "./dependencies/tuir.js";
import { patchConsole, WithProgress, WithConsole } from "./dependencies/ink-components.js";
import { Root } from "./Root.js";
import ansiEscapes from "ansi-escapes";

export function cli({ init }: { init?: () => void } = {}) {
  patchConsole();

  var root = (
    <WithProgress>
      <WithConsole autoRefreshInterval={1000}>
        <Root init={init} />
      </WithConsole>
    </WithProgress>
  )
  render(root, {
    patchConsole: false,
    debug: false,
    ansiEscapeChars: {
      clearScreen: ansiEscapes.clearScreen
    }
  });
}

export { commandEmitter, progressEmitter } from "./dependencies/ink-components.js";