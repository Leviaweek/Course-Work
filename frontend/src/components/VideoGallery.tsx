import { VideoCardModel } from "./VideoCardModel";
import { VideoCard } from "../models/VideoCard";
import "./VideoGallery.css";

const VideoGallery: React.FC<{onVideoClick: (videoCard: VideoCard) => void, videoList: VideoCard[]}> = ({onVideoClick, videoList}) => {
  return (
    <div className="VideoGallery">
      {videoList.map((video) => (
        <VideoCardModel
          key={video.id}
          videoCard={video}
          onVideoClick={onVideoClick}/>
      ))}
    </div>
  );
};

export { VideoGallery as VideoCards };