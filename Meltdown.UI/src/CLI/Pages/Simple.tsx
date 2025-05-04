import React, { useEffect } from "react";
import {
  ScrollingBox,
  progressEmitter,
  ProgressItemState,
  ProgressUpdate,
} from "../dependencies/ink-components.js";

export function SimpleLog() {
  const [log, setLog] = React.useState<string[]>(["nothing to show"]);
  useEffect(() => {
    const logListener = (path: string, message: string) => {
      setLog((prevLog) => [...prevLog, `L[${path}] ${message}`]);
    };
    const updateListener = (
      path: string,
      value: Partial<ProgressUpdate>
    ) => {
      setLog((prevLog) => [
        ...prevLog,
        `S[${path}] ${value.state}: ${value.status} (${value.details})`,
      ]);
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
