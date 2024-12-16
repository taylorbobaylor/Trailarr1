"use client";

import Image from "next/image";

interface MovieCardProps {
  id: number;
  title: string;
  year: number;
  posterPath?: string;
  trailerUrl?: string;
  isSelected: boolean;
  onSelect: (id: number) => void;
  downloadProgress?: number;
}

export default function MovieCard({
  id,
  title,
  year,
  posterPath,
  trailerUrl,
  isSelected,
  onSelect,
  downloadProgress,
}: MovieCardProps) {
  return (
    <div className="relative group">
      <div
        className={`relative aspect-[2/3] rounded-lg overflow-hidden border-2 transition-colors ${
          isSelected ? "border-theme-blue" : "border-transparent group-hover:border-theme-blue/50"
        }`}
      >
        {posterPath ? (
          <Image src={posterPath} alt={title} fill className="object-cover" />
        ) : (
          <div className="w-full h-full bg-theme-dark flex items-center justify-center">
            <span className="text-theme-light">No Poster</span>
          </div>
        )}

        {/* Download Progress Indicator */}
        {downloadProgress !== undefined && downloadProgress > 0 && downloadProgress < 100 && (
          <div className="absolute inset-0 bg-black/50 flex items-center justify-center">
            <div className="relative w-16 h-16">
              <svg className="w-full h-full" viewBox="0 0 36 36">
                <path
                  d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                  fill="none"
                  stroke="rgba(255, 255, 255, 0.2)"
                  strokeWidth="3"
                />
                <path
                  d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                  fill="none"
                  stroke="#fff"
                  strokeWidth="3"
                  strokeDasharray={`${downloadProgress}, 100`}
                />
                <text x="18" y="20" textAnchor="middle" fill="#fff" fontSize="8">
                  {`${downloadProgress}%`}
                </text>
              </svg>
            </div>
          </div>
        )}

        {/* Title and Year Overlay */}
        <div className="absolute inset-x-0 bottom-0 bg-gradient-to-t from-black/80 to-transparent p-4">
          <h3 className="text-white font-semibold truncate">{title}</h3>
          <p className="text-white/80 text-sm">{year}</p>
        </div>

        {/* Selection Checkbox */}
        <label className="absolute top-2 right-2 z-10">
          <input
            type="checkbox"
            className="sr-only peer"
            checked={isSelected}
            onChange={() => onSelect(id)}
          />
          <div
            className={`w-5 h-5 rounded border-2 transition-colors ${
              isSelected
                ? "bg-theme-blue border-theme-blue"
                : "border-white/50 peer-hover:border-white bg-black/30"
            }`}
          >
            {isSelected && (
              <svg
                className="w-full h-full text-white"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M5 13l4 4L19 7"
                />
              </svg>
            )}
          </div>
        </label>
      </div>

      {/* Trailer Status */}
      <div className="absolute inset-x-0 bottom-0 translate-y-full opacity-0 group-hover:opacity-100 transition-all pt-2">
        <div className="flex items-center justify-between text-sm text-theme-light">
          <span>{trailerUrl ? "Trailer Available" : "No Trailer"}</span>
        </div>
      </div>
    </div>
  );
}
