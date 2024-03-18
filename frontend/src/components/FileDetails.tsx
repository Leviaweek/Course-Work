import "./FileDetails.css"

const FileDetails: React.FC<{ file: File; }> = ({ file }) => {
  return (
    <div className="FileDetails">
      File details:
      <ul>
        <li>Name: {file.name}</li>
        <li>Type: {file.type}</li>
        <li>Size: {file.size} bytes</li>
      </ul>
    </div>
  );
};

export {FileDetails}