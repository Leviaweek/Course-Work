import { ConversionProgress } from "./ConversionProgress.";

type ConversionProgressDto = {
  progress: ConversionProgress | null;
  state: "Pending" | "InProgress" | "Completed" | "Failed";
};

export type { ConversionProgressDto };
