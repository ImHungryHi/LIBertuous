
          <article id="filter" class="is-fluid notification filter-is-fullwidth">
            <form action="{$formUrl|htmlentities}" enctype="multipart/form-data" method="post">
              <dl class="columns is-mobile is-multiline has-text-centered">
                <dd class="column is-hidden-mobile"></dd>
                <dd class="column is-narrow-tablet is-6-mobile">
                  <label for="selCategory">
                    <select id="selCategory" name="selCategory" onchange="submitForm()">
                      <option value="-1">Alle producten</option>

                      {iteration:iCategories}
                      <option value="{$categoryValue}"{option:oCategorySelected} selected{/option:oCategorySelected}>{$categoryText}</option>
                      {/iteration:iCategories}
                    </select>
                  </label>
                </dd>
                <dd class="column is-narrow-tablet is-3-mobile">
                  <label for="txtAlfabetSort">
                    A-Z
                    <a href="#" id="lnkSortAlfa" alt="Alfabetisch sorteren van {$alfaSortText}" onclick="sortAlfa()" class="{$txtAlfabetSort}">
                      <span class="icon"><i class="fa fa-{$alfaSortClass}"></i></span>
                    </a>
                  </label>
                </dd>
                <dd class="column is-narrow-tablet is-3-mobile">
                  <label for="txtPriceSort">
                    &euro;
                    <a href="#" id="lnkSortPrice" alt="Prijs sorteren van {$priceSortText}" onclick="sortPrice()" class="{$txtPriceSort}">
                      <span class="icon"><i class="fa fa-{$priceSortClass}"></i></span>
                    </a>
                  </label>
                </dd>
                <dd class="column is-narrow-tablet is-12-mobile">
                  <label for="txtFilterText">
                    <input type="text" id="txtFilterText" name="txtFilterText" value="{$txtFilterText}" onkeyup="filterByText()" placeholder="Zoeken" />
                  </label>
                </dd>
                <dd class="column is-hidden-mobile"></dd>
                <dd class="hidden">
                  <label for="btnSubmit"><button id="btnSubmit" name="action" value="submit">Filteren</button></label>
                  <label for="btnSortAlfa"><button id="btnSortAlfa" name="action" value="sortAlfa">Alfabetisch sorteren</button></label>
                  <label for="btnSortPrice"><button id="btnSortPrice" name="action" value="sortPrice">Sorteren op prijs</button></label>
                  <input type="text" id="txtAlfabetSort" name="txtAlfabetSort" value="{$txtAlfabetSort}" />
                  <input type="text" id="txtPriceSort" name="txtPriceSort" value="{$txtPriceSort}" />
                </dd>
              </dl>
            </form>
          </article>

          <section id="cardWrapper" class="section">
            <div class="container">
              <div class="columns is-multiline">
                {iteration:iItems}
                <article class="column is-one-third" id="item{$itemId}">
                  <div class="card">
                    <figure class="image is-4by3">
                      <img src="{$imgUrl|htmlentities}" alt="{$imgAlt|htmlentities}" />
                    </figure>

                    <div class="content">
                      <h2 class="title">{$itemTitle|htmlentities}</h2>
                      {option:oHasQuantifier}<p><span>{$itemQuantifier}</span></p>{/option:oHasQuantifier}
                      {option:oHasDescription}<p>{$itemDescription}</p>{/option:oHasDescription}
                    </div>

                    <div class="bottomline">
                      <p id="priceSpan_{$itemId}" class="price-span">&euro; {$price}</p>

                      <footer class="card-footer is-12">
                        <div class="card-footer-item">
                          <label for="txtQuantity">
                            <input type="number" name="txtQuantity" id="txtQuantity_{$itemId}" min="1" max="1000" value="1" class="is-number input is-small" />
                          </label>
                        </div>
                        {option:oHasOptions}
                        {iteration:iOptions}
                        {option:oNewOptionRow}
                      </footer>
                      <footer class="card-footer is-12">
                        {/option:oNewOptionRow}
                        {option:oOptionIsCheckbox}
                        <div class="card-footer-item">
                          <label for="chkOption{$optionId}_{$itemId}" class="checkbox">
                            <input type="checkbox" id="chkOption{$optionId}_{$itemId}" name="chkOption{$optionId}_{$itemId}" value="{$optionValue}" {option:oOptionChecked}checked {/option:oOptionChecked}/>
                            {$optionText}
                          </label>
                        </div>
                        {/option:oOptionIsCheckbox}
                        {option:oOptionIsDropdown}
                        <div class="card-footer-item is-10">
                          <label for="selOption{$optionId}_{$itemId}">
                            <select id="selOption{$optionId}_{$itemId}" name="selOption{$optionId}_{$itemId}" class="select is-small">
                              {option:oIsNotMandatory}<option value="0" selected>{$defaultOptionText}</option>{/option:oIsNotMandatory}
                              {iteration:iOptionItems}
                              <option value="{$optionValue}"{option:oOptionSelected} selected{/option:oOptionSelected}>{$optionText}</option>
                              {/iteration:iOptionItems}
                            </select>
                          </label>
                        </div>
                        {/option:oOptionIsDropdown}
                        {/iteration:iOptions}
                        {/option:oHasOptions}
                      </footer>

                      <footer class="card-footer">
                        <div class="message notification is-primary has-text-centered is-fluid filter-is-fullwidth">
                          <a href="javascript:void(0);" id="lnkAddToCart_{$itemId}" onclick="sendItem({$itemId})"><i class="fa fa-plus">&nbsp;&nbsp;&nbsp;</i>Toevoegen aan winkelwagentje</a>
                        </div>
                      </footer>
                    </div>
                  </div>
                </article>
                {/iteration:iItems}
              </div>
            </div>
          </section>
