import "./VideoFIleInput.css"

const VideoFileInput: React.FC<{ onChangeFile: (file: File) => void; }> = ({
  onChangeFile,
}) => {
  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    files && files[0] && onChangeFile(files[0]);
  };
  return (
    <div className="VideoFileInput">
      <label htmlFor="file" className="sr-only">
        Choose a file: 
      </label>
      <input id="file" type="file" accept=".mp4" onChange={handleFileChange} />
    </div>
  );
};

export { VideoFileInput };
