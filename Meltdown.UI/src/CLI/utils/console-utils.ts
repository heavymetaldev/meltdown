import { EventEmitter } from "node:events";
import { format } from "node:util";

const consoleMethods = [
  "assert",
  "count",
  "countReset",
  "debug",
  "dir",
  "dirxml",
  "error",
  "group",
  "groupCollapsed",
  "groupEnd",
  "info",
  "log",
  "table",
  "time",
  "timeEnd",
  "timeLog",
  "trace",
  "warn",
];

type Callback = (stream: string, data: string) => void;

export const ConsoleEmitter: EventEmitter = new EventEmitter();

export function patchConsole(callback?: Callback) {
  callback ??= (stream, data) => {
    ConsoleEmitter.emit("console", stream, data);
  };

  for (const method of consoleMethods) {
    (console as any)[method] = (...data: any[]) => callback(method, format.apply(null, data));
  }
}
