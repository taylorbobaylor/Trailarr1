'use client';

import { useEffect, useState } from 'react';
import MovieCard from '@/components/MovieCard';

interface Movie {
  tmdbId: number;
  title: string;
  year: number;
  posterPath: string;
  hasTrailer: boolean;
}

export default function Home() {
  const [movies, setMovies] = useState<Movie[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedMovies, setSelectedMovies] = useState<Set<number>>(new Set());

  useEffect(() => {
    const fetchMovies = async () => {
      try {
        const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/movies`);
        if (!response.ok) throw new Error('Failed to fetch movies');
        const data = await response.json();
        setMovies(data);
      } catch (error) {
        console.error('Failed to fetch movies:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchMovies();
  }, []);

  const handleMovieSelect = (tmdbId: number) => {
    setSelectedMovies(prev => {
      const newSet = new Set(prev);
      if (newSet.has(tmdbId)) {
        newSet.delete(tmdbId);
      } else {
        newSet.add(tmdbId);
      }
      return newSet;
    });
  };

  const handleDownloadTrailers = async () => {
    try {
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/movies/download-trailers`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(Array.from(selectedMovies)),
      });
      if (!response.ok) throw new Error('Failed to download trailers');
      setSelectedMovies(new Set());
    } catch (error) {
      console.error('Failed to download trailers:', error);
    }
  };

  return (
    <main className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold">Your Movies</h1>
        {selectedMovies.size > 0 && (
          <button
            onClick={handleDownloadTrailers}
            className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded-md"
          >
            Download Selected Trailers ({selectedMovies.size})
          </button>
        )}
      </div>

      {loading ? (
        <div className="flex justify-center">
          <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-white"></div>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
          {movies.map((movie) => (
            <MovieCard
              key={movie.tmdbId}
              {...movie}
              selected={selectedMovies.has(movie.tmdbId)}
              onSelect={() => handleMovieSelect(movie.tmdbId)}
            />
          ))}
        </div>
      )}
    </main>
  );
}
