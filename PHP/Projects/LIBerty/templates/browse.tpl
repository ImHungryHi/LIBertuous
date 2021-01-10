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
							{option:oNotOwned}
							<a href="/move.php?id={$iId}&to=owned">Move to owned list</a>
							{/option:oNotOwned}
							{option:oNotWishlisted}
							<a href="/delete.php?id={$iId}&category=wishlist">Move to wishlist</a>
							{/option:oNotWishlisted}
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
						<p>Oops, we've got no movies to show. Has anyone even filmed anything?</p>
						<p>Grab the camera, quickly!</p>
					</div>
					
					{/option:oNoMovies}