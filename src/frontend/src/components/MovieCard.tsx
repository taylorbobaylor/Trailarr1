'use client';

import React from 'react';
import Image from 'next/image';

interface MovieCardProps {
  tmdbId: number;
  title: string;
  year: number;
  posterPath: string;
  hasTrailer: boolean;
  selected: boolean;
  onSelect: (tmdbId: number) => void;
}

export default function MovieCard({
  tmdbId,
  title,
  year,
  posterPath,
  hasTrailer,
  selected,
  onSelect,
}: MovieCardProps) {
  return (
    <div
      className={`relative bg-gray-800 rounded-lg overflow-hidden shadow-lg cursor-pointer transition-transform hover:scale-105 ${
        selected ? 'ring-2 ring-blue-500' : ''
      }`}
      onClick={() => onSelect(tmdbId)}
    >
      <div className="absolute top-2 right-2 z-10">
        <input
          type="checkbox"
          checked={selected}
          onChange={() => onSelect(tmdbId)}
          className="h-5 w-5 rounded border-gray-300"
        />
      </div>

      {posterPath ? (
        <div className="relative h-[300px]">
          <Image
            src={`https://image.tmdb.org/t/p/w500${posterPath}`}
            alt={title}
            fill
            className="object-cover"
            unoptimized
          />
        </div>
      ) : (
        <div className="w-full h-[300px] bg-gray-700 flex items-center justify-center">
          No Poster
        </div>
      )}

      <div className="p-4">
        <h2 className="text-lg font-semibold text-white">{title}</h2>
        <p className="text-gray-400">{year}</p>
        <div className="mt-2">
          {hasTrailer ? (
            <span className="text-green-400">Has Trailer</span>
          ) : (
            <span className="text-yellow-400">No Trailer</span>
          )}
        </div>
      </div>
    </div>
  );
}
