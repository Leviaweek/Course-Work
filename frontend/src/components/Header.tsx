import "./Header.css"

interface HeaderProps {
  showUploadModal: () => void;
  toggleTheme: () => void;
}

const Header: React.FC<HeaderProps> = ({showUploadModal, toggleTheme}) => {
  return (
    <div className="Header">
      <button onClick={showUploadModal}>Add Video</button>
      <button onClick={toggleTheme}>SwitchTheme</button>
    </div>
  );
};

export { Header };
