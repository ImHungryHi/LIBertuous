					{option:oMovies}
					<section id="movies">
						{iteration:iMovies}
						<article class="movie">
							<img src="{$iThumbPath|htmlentities}" alt="{$iTitle}" />
							<h1>{$iTitle}</h1>
							{iteration:iDirectors}
							<p class="directors"><a href="/wishlist.php?director={$iDirId}">{$iDirector}</a></p>
							{/iteration:iDirectors}
							<p>{$iYear}</p>
							{iteration:iGenres}
							<p class="genres"><a href="/wishlist.php?genre={$iGenreId}">{$iGenre}</a></p>
							{/iteration:iGenres}
							<a href="/move.php?id={$iId}&to=owned">Move to owned list</a>
							<a href="/delete.php?id={$iId}&category=wishlist">Delete from wishlist</a>
						</article>
						{/iteration:iMovies}
					</section>
					{option:oPaging}
					<nav id="paging">
						<ul>
							<li><a href="#">Previous</a></li>
							{iteration:iPaging}
							<li><a href="#">{$iPageNumber}</a></li>
							{/iteration:iPaging}
							<li><a href="#">Next</a></li>
						</ul>
					</nav>
					{/option:oPaging}
					
					{/option:oMovies}
					{option:oNoMovies}
					<div id="boxEmpty">
						<p>You've got no movies in your wishlist yet, try picking a few in the <a href="/browse.php">browse</a> page</p>	
					</div>
					
					{/option:oNoMovies}