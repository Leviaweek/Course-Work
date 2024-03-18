import { useEffect } from "react";
import { LoadingState } from "../models/LoadingState";

const UploadFailed: React.FC<{
  changeState: (state: LoadingState) => void;
}> = ({ changeState }) => {
  useEffect(() => {
    setTimeout(() => {
      changeState("Form");
    }, 5000);
  }, [changeState]);
  return <p>Upload failed</p>;
};

export { UploadFailed };