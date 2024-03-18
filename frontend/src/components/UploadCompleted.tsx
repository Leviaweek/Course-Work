import { useEffect } from "react";
import { LoadingState } from "../models/LoadingState";

const UploadCompleted: React.FC<{ 
  changeState: (state: LoadingState, ) => void;
  updateVideos: () => void;
}> = ({ changeState, updateVideos }) => {
  useEffect(() => {
      updateVideos();
    setTimeout(() => {
      changeState("Form");
    }, 5000);
  }, [changeState, updateVideos]);
  return <p>Upload completed</p>;
};

export { UploadCompleted };