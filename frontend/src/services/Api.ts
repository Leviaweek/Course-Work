import { ConversionProgressDto } from "../models/ConversionProgressDto";
import { VideoResponse } from "../models/VideoResponse";

class Api
{
    public static getVideosUrl(): string
    {
        const url: string = import.meta.env.VITE_API_URL;
        return `${url}/api/videos`
    }
    public static getPreviewUrl(id: string): string
    {
        const url: string = import.meta.env.VITE_API_URL;
        return `${url}/api/videos/${id}/preview`
    }
    public static getVideoUrl(id: string): string
    {
        const url: string = import.meta.env.VITE_API_URL;
        return `${url}/api/videos/${id}`
    }
    public static async fetchVideosAsync(): Promise<VideoResponse>
    {
        const response = await fetch(this.getVideosUrl());
        const data = await response.json();
        return data;
    }
    public static async uploadTitleAsync(title: string): Promise<string>
    {
        const formData = new FormData();
        formData.append("title", title);
        const response = await fetch(this.getVideosUrl(), {
            method: "POST",
            body: formData,
        });
        const data = await response.json();
        return data;
    }

    public static async uploadVideoAsync(file: File, id: string): Promise<string>
    {
        const formData = new FormData();
        formData.append("formFile", file);
        const response = await fetch(`${this.getVideosUrl()}/${id}`, {
            method: "POST",
            body: formData,
        });
        const data = await response.json();
        return data;
    }
    public static async getUploadStatusAsync(id: string): Promise<ConversionProgressDto>
    {
        console.log(id)
        const response = await fetch(`${this.getVideosUrl()}/${id}/status`);
        const data = await response.json();
        return data;
    }

}
export { Api }