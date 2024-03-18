import "./Header.css"

const Header: React.FC<{showUploadModal: () => void, toggleTheme: () => void}> = ({showUploadModal, toggleTheme}) => {
  return (
    <div className="Header">
      <button onClick={showUploadModal}>Add Video</button>
      <button onClick={toggleTheme}>SwitchTheme</button>
    </div>
  );
};

export { Header };
