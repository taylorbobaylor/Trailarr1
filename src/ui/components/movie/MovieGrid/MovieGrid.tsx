interface Movie { id: number; title: string; year: number; posterPath?: string; trailerUrl?: string; } interface MovieGridProps { movies: Movie[]; selectedMovies: number[]; onMovieSelect: (movieId: number) => void; onSelectAll: () => void; onDownloadTrailers: () => void; } export default function MovieGrid({ movies, selectedMovies, onMovieSelect, onSelectAll, onDownloadTrailers }: MovieGridProps) { return ( <div className="p-6"> <div className="flex items-center justify-between mb-6"> <h1 className="text-2xl font-semibold text-theme-light">Movies</h1> <div className="flex items-center space-x-4"> <button onClick={onSelectAll} className="btn btn-primary"> {selectedMovies.length === movies.length ? "Deselect All" : "Select All"} </button> <button onClick={onDownloadTrailers} disabled={selectedMovies.length === 0} className="btn btn-primary disabled:opacity-50 disabled:cursor-not-allowed"> Download Selected Trailers </button> </div> </div> <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-6"> {movies.map((movie) => ( <MovieCard key={movie.id} movie={movie} isSelected={selectedMovies.includes(movie.id)} onSelect={() => onMovieSelect(movie.id)} /> ))} </div> </div> ); }
