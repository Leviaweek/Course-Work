import React from "react";
import { Api } from "../services/Api";
import { VideoCard } from "../models/VideoCard";
import "./VideoCardModel.css";
import moment from "moment";

interface VideoCardModelProps {
  videoCard: VideoCard;
  onVideoClick: (videoCard: VideoCard) => void;
}

const VideoCardModel: React.FC<VideoCardModelProps> = ({ videoCard, onVideoClick }): JSX.Element => {
  const format = moment.utc(videoCard.duration).format("HH:mm:ss").replace(/^00:/g, "");
  return (
    <div className="VideoCardModel" onClick={() => onVideoClick(videoCard)}>
      <div className="image-container">
        <img src={Api.getPreviewUrl(videoCard.id)}></img>
        <div className="duration">
          <p>{format}</p>
        </div>
      </div>  
      <div className="video-info">
        <h1>{videoCard.title}</h1>
        <p>{moment(videoCard.createdAt).fromNow()}</p>
      </div>
    </div>
  );
};
export { VideoCardModel };
export type { VideoCard };
