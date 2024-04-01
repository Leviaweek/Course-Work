import { VideoCardModel } from "./VideoCardModel";
import { VideoCard } from "../models/VideoCard";
import "./VideoGallery.css";

interface VideoGalleryProps {
  onVideoClick: (videoCard: VideoCard) => void;
  videoList: VideoCard[];
}

const VideoGallery: React.FC<VideoGalleryProps> = ({
  onVideoClick,
  videoList,
}) => {
  return (
    <div className="VideoGallery">
      {videoList.map((video) => (
        <VideoCardModel
          key={video.id}
          videoCard={video}
          onVideoClick={onVideoClick}
        />
      ))}
    </div>
  );
};

export { VideoGallery as VideoCards };
