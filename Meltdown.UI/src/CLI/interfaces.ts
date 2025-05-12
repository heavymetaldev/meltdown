import type { CommandEmitter, IProgressEmitter, ProgressUpdate } from "./dependencies/ink-components.js";

export type CliExport = {
  cli: (params: any) => void;
  commandEmitter: CommandEmitter;
  progressEmitter: IProgressEmitter;
};