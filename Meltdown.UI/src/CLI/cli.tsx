import React from "react";

import { render } from "./dependencies/tuir.js";
import {
  patchConsole,
  WithProgress,
  WithConsole,
  commandEmitter,
  progressEmitter,
} from "./dependencies/ink-components.js";
import { Root } from "./Root.js";
import ansiEscapes from "ansi-escapes";
import { CliExport } from "./interfaces.js";

export function cli(params: Parameters<typeof Root>[0]) {
  patchConsole();

  process.stdout.write("entering ALTBUF\n");
  // Enable alternative screen buffer
  process.stdout.write(ansiEscapes.enterAlternativeScreen);
  process.stdout.write(ansiEscapes.cursorSavePosition);

  // Restore main screen buffer on exit
  const cleanup = () => {
    process.stdout.write(ansiEscapes.exitAlternativeScreen);
    process.stdout.write("\nexited ALTBUF\n");
  };
  process.on('exit', cleanup);
  process.on('SIGINT', () => {
    cleanup();
    process.exit();
  });

  
  var root = (
    <WithProgress>
      <WithConsole autoRefreshInterval={1000}>
        <Root {...params} />
      </WithConsole>
    </WithProgress>
  );
  render(root, {
    patchConsole: false,
    debug: false,
    //allowHalfOpen: false,
    throttle: 30,
    ansiEscapeChars: {
       clearScreen: ansiEscapes.clearTerminal // <-- this causes each frame to stay in the terminal history
      //clearScreen: ansiEscapes.clearScreen,
    },
  });
}

const exportedCli: CliExport = {
  cli,
  commandEmitter,
  progressEmitter,
};

export default exportedCli;
