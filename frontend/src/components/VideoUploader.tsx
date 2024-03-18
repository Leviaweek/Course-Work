import { useState } from "react";
import { UploadStatus } from "./UploadStatus";
import { FileUploadForm } from "./FileUploadForm";
import { LoadingState } from "../models/LoadingState";
import { UploadCompleted } from "./UploadCompleted";
import { UploadFailed } from "./UploadFailed";

const VideoUploader: React.FC<{ updateVideos: () => void}> = ({updateVideos}): JSX.Element => {
  const [loadingState, setLoadingState] = useState<LoadingState>("Form");
  const [statusId, setStatusId] = useState<string>("");
  switch (loadingState) {
    case "Form":
      return (
        <FileUploadForm
          changeState={setLoadingState}
          setStatusId={setStatusId}
        />
      );
    case "InProgress":
      return <UploadStatus statusId={statusId} changeState={setLoadingState} />;
    case "Completed":
      return <UploadCompleted changeState={setLoadingState} updateVideos={updateVideos} />;
    case "Failed":
      return <UploadFailed changeState={setLoadingState} />;
  }
};

export { VideoUploader };
