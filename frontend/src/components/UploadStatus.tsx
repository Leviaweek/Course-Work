import { useCallback, useEffect, useState } from "react";
import { Api } from "../services/Api";
import { ConversionProgressModel } from "./ConversionProgressModel";
import { ConversionProgressDto } from "../models/ConversionProgressDto";
import { LoadingState } from "../models/LoadingState";

interface UploadStatusProps {
  statusId: string | null;
  changeState: (state: LoadingState) => void;
}

const UploadStatus: React.FC<UploadStatusProps> = ({ statusId, changeState }) => {
  const [progress, setProgress] = useState<ConversionProgressDto | null>(null);

  const fetchStatus = async () => {
    if (!statusId) {
      return;
    }
    const uploadStatus = await Api.getUploadStatusAsync(statusId);
    setProgress(uploadStatus);
    switch (uploadStatus.state) {
      case "Completed":
        changeState("Completed");
        break;
      case "Failed":
        changeState("Failed");
        break;
    }
  };
    const fetchCallback = useCallback(fetchStatus, [changeState, statusId]);
  useEffect(() => {
    const interval = setInterval(() => {
      fetchCallback();
    }, 1000);

    return () => clearInterval(interval);
  }, [fetchCallback]);

  if (!progress) {
    return <p>Fetching status...</p>;
  }

  return <ConversionProgressModel uploadProgress={progress} />;
};

export { UploadStatus };
