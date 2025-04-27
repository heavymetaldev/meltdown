import { EventEmitter } from "node:events";

declare interface CommandEmitter {
  on(event: 'invoke', listener: (command: string, path: string, args: string[]) => void): this;
}

class CommandEmitter extends EventEmitter {
  invokeCommand(command: string, path: string, args: string[]): void {
    this.emit('invoke', command, path, args);
  }
}

export const commandEmitter: CommandEmitter = new CommandEmitter();

export type ProgressItemState =
  | "new"
  | "pending"
  | "starting"
  | "running"
  | "stopped"
  | "done"
  | "error"
  | "connected"
  | "disconnected"
  | "unknown";

declare interface ProgressEmitter {
  on(event: "log", listener: (path: string, message: string) => void): this;
  on(
    event: "update",
    listener: (
      path: string,
      state: ProgressItemState,
      status?: string,
      details?: string
    ) => void
  ): this;
}

class ProgressEmitter extends EventEmitter {
  log(path: string, message: string): void {
    this.emit("log", path, message);
  }

  update(
    path: string,
    state: ProgressItemState,
    status?: string,
    details?: string
  ): void {
    this.emit("update", path, state, status, details);
  }
}

export const progressEmitter: ProgressEmitter = new ProgressEmitter();
