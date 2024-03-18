import { VideoCard } from "../models/VideoCard";
import { Api } from "../services/Api";
import "./VideoModel.css";

const VideoModel: React.FC<{ video: VideoCard; }> = ({ video }) => {
  return (
    <div className="VideoModel">
      <video
        src={Api.getVideoUrl(video.id)}
        controls autoPlay />
    </div>
  );
};

export { VideoModel };