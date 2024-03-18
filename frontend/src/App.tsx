import "./App.css";
import { VideoCards as VideoGallery } from "./components/VideoGallery";
import Modal from "react-modal";
import { useCallback, useEffect, useState } from "react";
import { VideoCard } from "./models/VideoCard";
import { Api } from "./services/Api";
import { Header } from "./components/Header";
import { VideoUploader } from "./components/VideoUploader";
import { VideoModel } from "./components/VideoModel";
import { useLocalStorage } from "usehooks-ts";


function App() {
  const [theme, setTheme] = useLocalStorage<"light-theme" | "dark-theme">("theme", "light-theme");
  const toggleTheme = () => {
    setTheme(theme === "light-theme" ? "dark-theme" : "light-theme");
  };
  const [videoModalIsOpen, setVideoModalIsOpen] = useState<boolean>(false);
  const [currentVideo, setCurrentVideo] = useState<VideoCard | null>(null);

  const [uploadModalIsOpen, setUploadModalIsOpen] = useState<boolean>(false);

  const [videoList, setVideoList] = useState<VideoCard[]>([]);
  const fetchVideos = async () => {
    const data = await Api.fetchVideosAsync();
    setVideoList(data.videos);
  };
  const fetchVideosCallback = useCallback(fetchVideos, []);
  useEffect(() => {
    fetchVideos();
  }, [fetchVideosCallback]);

  const showUploadModal = () => {
    setUploadModalIsOpen(true);
  };
  const hideUploadModal = () => {
    setUploadModalIsOpen(false);
  };

  const onVideoClick = (videoCard: VideoCard) => {
    showVideoModal();
    setCurrentVideo(videoCard);
  };

  const showVideoModal = () => {
    setVideoModalIsOpen(true);
  };

  const hideVideoModal = () => {
    setCurrentVideo(null);
    setVideoModalIsOpen(false);
  };

  return (
    <div className={`App ${theme}`}>
      <Header showUploadModal={showUploadModal} toggleTheme={toggleTheme}/>
      <VideoGallery onVideoClick={onVideoClick} videoList={videoList} />
      <Modal className={`video-player ${theme}`}
        isOpen={videoModalIsOpen}
        onAfterOpen={showVideoModal}
        onAfterClose={hideVideoModal}
        onRequestClose={hideVideoModal}
        shouldCloseOnOverlayClick={true}
      >
        {currentVideo && (
          <div className="video-container">
            <VideoModel video={currentVideo} />
          </div>
        )}
      </Modal>
      <Modal className={`upload-modal ${theme}`}
        isOpen={uploadModalIsOpen}
        onAfterOpen={showUploadModal}
        onAfterClose={hideUploadModal}
        onRequestClose={hideUploadModal}
        shouldCloseOnOverlayClick={true}
      >
        <VideoUploader updateVideos={() => fetchVideosCallback()} />
      </Modal>
    </div>
  );
}

export default App;
