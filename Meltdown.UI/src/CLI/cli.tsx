import React from "react";

import { render } from "tuir";
import { patchConsole } from "./utils/console-utils.js";
import { Root } from "./Root.js";


export default function cli() {

  patchConsole();

  render(<Root/>, {
    patchConsole: true,
  });
}
