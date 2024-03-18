import { ConversionProgressDto } from "../models/ConversionProgressDto";

const ConversionProgressModel: React.FC<{uploadProgress: ConversionProgressDto}> = ({
    uploadProgress
}): JSX.Element => {
  return (
    <div>
      <p>State: {uploadProgress.state}</p>
      {uploadProgress.progress && (
        <>
          <p>Speed: {uploadProgress.progress.speed}</p>
          <p>Last update: {uploadProgress.progress.lastUpdate}</p>
          <p>Progress: {uploadProgress.progress.percent}%</p>
          <p>CurrentProgress: {uploadProgress.progress.progress}</p>
        </>
      )}
    </div>
  );
};

export { ConversionProgressModel };