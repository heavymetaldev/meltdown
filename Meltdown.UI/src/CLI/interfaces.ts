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

type ProgressUpdate = {
  state: ProgressItemState;
  status: string;
  details?: string;
  progress?: number;
};

interface ProgressEmitter {
  log(path: string, message: string): void;

  update(path: string, value: Partial<ProgressUpdate>): void;

  command(path: string, command: string, argsStr: string): void;
  //   on(event: "log", listener: (path: string, message: string) => void): this;
  //   on(
  //     event: "update",
  //     listener: (path: string, value: Partial<ProgressUpdate>) => void
  //   ): this;
  //   on(
  //     event: "command",
  //     listener: (command: string, path: string, args: string[]) => void
  //   ): this;
}

interface CommandEmitter {
  on(
    event: "invoke",
    listener: (command: string, path: string, args: string[]) => void
  ): this;
}

export type CliExport = {
  cli: (params: any) => void;
  commandEmitter: CommandEmitter;
  progressEmitter: ProgressEmitter;
};
