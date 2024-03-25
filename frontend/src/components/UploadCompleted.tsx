import { useEffect } from "react";
import { LoadingState } from "../models/LoadingState";

interface UploadCompletedProps {
  changeState: (state: LoadingState, ) => void;
  updateVideos: () => void;
}

const UploadCompleted: React.FC<UploadCompletedProps> = ({ changeState, updateVideos }) => {
  useEffect(() => {
      updateVideos();
    setTimeout(() => {
      changeState("Form");
    }, 5000);
  }, [changeState, updateVideos]);
  return <p>Upload completed</p>;
};

export { UploadCompleted };