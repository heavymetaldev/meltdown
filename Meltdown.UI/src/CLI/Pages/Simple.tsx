import React, { useEffect } from "react";
import {
  ScrollingBox,
  progressEmitter,
  ProgressItemState,
} from "../ink-components.js";

export function SimpleLog() {
  const [log, setLog] = React.useState<string[]>(["nothing to show"]);
  useEffect(() => {
    const logListener = (path: string, message: string) => {
      setLog((prevLog) => [...prevLog, `L[${path}] ${message}`]);
    };
    const updateListener = (
      path: string,
      state: ProgressItemState,
      status?: string,
      details?: string
    ) => {
      setLog((prevLog) => [...prevLog, `S[${path}] ${state}: ${status} (${details})`]);
    };

    progressEmitter.on("log", logListener);
    progressEmitter.on("update", updateListener);

    return () => {
      progressEmitter.off("log", logListener);
      progressEmitter.off("update", updateListener);
    };
  }, []);

  return <ScrollingBox data={log} />;
}
