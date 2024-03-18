import { useState } from "react";
import { Api } from "../services/Api";
import { LoadingState } from "../models/LoadingState";
import { FileDetails } from "./FileDetails";
import { VideoFileInput } from "./VideoFileInput";
import "./FileUploadForm.css";

const FileUploadForm: React.FC<{
  changeState: (state: LoadingState) => void;
  setStatusId: (id: string) => void;
}> = ({ changeState, setStatusId }) => {

  const [file, setFile] = useState<File | null>(null);
  const [tempInputValue, setTempInputValue] = useState<string>("");

  const onInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setTempInputValue(e.target.value);
  };

  const handleUpload = async () => {
    if (!file) {
      return;
    }
    const uploadVideoId = await Api.uploadTitleAsync(tempInputValue);
    const statusId = await Api.uploadVideoAsync(file, uploadVideoId);
    setStatusId(statusId);
    changeState("InProgress");
  };
  return (
    <div className="FileUploadForm">
      {file && <FileDetails file={file} />}
      <VideoFileInput onChangeFile={setFile} />


      <label htmlFor="input">Title</label>
      <input
        type="text"
        onChange={onInputChange}
        value={tempInputValue}
      ></input>
      {(file && tempInputValue !== "" ) && (
        <button type="button" onClick={handleUpload}>
          Upload a file
        </button>
      )}
    </div>
  );
};

export { FileUploadForm };
