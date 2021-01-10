
          <div class="container">
            <form action="{$formUrl|htmlentities}" enctype="multipart/form-data" method="post">
              <a onclick="updateItems()">&lt; Verderwinkelen</a>
              <h2 class="title is-2">Winkelwagentje</h2>

              {option:oHasNoItems}
              <div class="notification has-text-centered">
                <h3 class="subtitle is-4">Hier is niets te zien, wil je <a onclick="updateItems()">verderwinkelen</a>?</h3>
                <dl class="hidden">
                  <dd>
                    <label for="btnUpdate"><button id="btnUpdate" name="action" value="updateAndBack">Verderwinkelen</button></label>
                  </dd>
                </dl>
              </div>
              {/option:oHasNoItems}
              {option:oHasItems}
              <div class="columns is-multiline is-12 is-gapless">
                <div class="column hero has-border-bottom is-12 parentColumn titleColumn">
        					<div class="columns is-gapless level">
        						<div class="column level-item is-12-mobile">
        							&nbsp;
        						</div>
        						<div class="column level-item">
        							<p>Info</p>
        						</div>
        						<div class="column level-item has-text-centered">
        							<p>Prijs</p>
        						</div>
        						<div class="column level-item has-text-centered">
        							<p>Aantal</p>
        						</div>
        						<div class="column level-item has-text-centered">
        							<p>Subtotaal</p>
        						</div>
        						<div class="column level-item has-text-centered is-narrow is-1">
        							&nbsp;
        						</div>
                	</div>
                </div>

                <div class="column hero has-border-bottom is-12 parentColumn">
                  {iteration:iItems}
                  <div class="columns is-gapless level grid-box childColumn">
        						<div class="column level-item is-12-mobile grid-1">
                      <figure class="image is-128x128"><img src="modules/shopcart/img/placeholder.jpg" alt="placeholder" /></figure>
        						</div>

                    <div class="column level-item grid-2">
                      <h2 class="title is-4">{$artTitle}</h2>
                      {option:oHasQuantifier}<p><span>{$itemQuantifier}</span></p>{/option:oHasQuantifier}
                      {option:oHasDescription}<p>{$itemDescription}</p>{/option:oHasDescription}
                      {option:oHasOptions}
                      {iteration:iOptions}
                      <div class="tile">
                      {option:oOptionIsCheckbox}
                        <label for="chkOption{$optionId}_{$itemId}" class="checkbox">
                          <input type="checkbox" id="chkOption{$optionId}_{$itemId}" name="chkOption{$optionId}_{$itemId}" onchange="updateItem(&quot;{$itemId}&quot;)" value="{$optionValue}" {option:oOptionChecked}checked {/option:oOptionChecked}/>
                          {$optionText}{option:oOptionHasCost} (+ &euro; <span id="extraPrice{$optionId}_{$itemId}">{$optionPrice}</span>){/option:oOptionHasCost}
                      {/option:oOptionIsCheckbox}
                      {option:oOptionIsDropdown}
                        <label for="selOption{$optionId}_{$itemId}">
                          <select id="selOption{$optionId}_{$itemId}" name="selOption{$optionId}_{$itemId}" onchange="updateItem(&quot;{$itemId}&quot;)" class="select is-small">
                            {option:oIsNotMandatory}<option value="0"{option:oNoneSelected} selected{/option:oNoneSelected}>{$defaultOptionText}</option>{/option:oIsNotMandatory}
                            {iteration:iOptionItems}
                            <option value="{$optionValue}"{option:oOptionSelected} selected{/option:oOptionSelected}>{$optionText}{option:oOptionHasPrice} &ndash; &euro; {$optionPrice}{/option:oOptionHasPrice}</option>
                            {/iteration:iOptionItems}
                          </select>
                      {/option:oOptionIsDropdown}
                        </label>
                      </div>
                      {/iteration:iOptions}
                      {/option:oHasOptions}
                    </div>

                    <div class="column level-item has-text-centered grid-3">
                      <strong>&euro; <span id="itemPrice&lowbar;{$itemId}">{$artPrice}</span><span class="informators"> / eenheid</span></strong>
                    </div>

                    <div class="column level-item has-text-centered is-5-mobile grid-4">
                      <label for="selQuantityFor&lowbar;{$itemId}"><input type="number" id="selQuantityFor&lowbar;{$itemId}" name="selQuantityFor&lowbar;{$itemId}" class="is-number input is-small itemQuantity" min="1" max="1000" value="{$artQuantity}" onchange="updateItem(&quot;{$itemId}&quot;)" /></label>
                    </div>

                    <div class="column level-item has-text-centered grid-5">
                      <p><strong><span class="informators">Totaal: </span><span id="itemTotal&lowbar;{$itemId}" class="itemTotal">&euro; {$artTotal}</span></strong></p>
                    </div>

                    <div class="column level-item has-text-centered is-narrow is-1 grid-6">
                      <a class="button is-danger" onclick="deleteArticle(&quot;{$itemId}&quot;)" data-placement="bottom" data-toggle="tooltip" title="Verwijderen"><i class="fa fa-trash"></i></a>
                      <label for="btnDeleteFor&lowbar;{$itemId}"><button id="btnDeleteFor&lowbar;{$itemId}" name="action" class="hidden" value="btnDeleteFor&lowbar;{$itemId}">Verwijderen</button></label>
                    </div>
                  </div>
                  {/iteration:iItems}
                </div>

                <div class="column hero has-border-bottom is-12 parentColumn">
        					<div class="columns is-gapless level">
        						<div class="column level-item is-12-mobile">
        							&nbsp;
        						</div>
        						<div class="column level-item is-hidden-mobile">
        							&nbsp;
        						</div>
        						<div class="column level-item is-hidden-mobile">
        							&nbsp;
        						</div>
        						<div class="column level-item has-text-centered is-5-mobile is-left-mobile">
        							<strong>Totaal:</strong>
        						</div>
        						<div class="column level-item has-text-centered is-5-mobile is-right-mobile">
        							<strong><span id="grandTotal">&euro; {$shopcartTotal}</span></strong>
        						</div>
        						<div class="column level-item has-text-centered is-narrow is-1 is-hidden-mobile">
        							&nbsp;
        						</div>
                	</div>
                </div>
              </div>

              <div class="bottomPart content">
                <div id="comments">
                  <label for="txtComment"><textarea id="txtComment" class="textarea" name="txtComment" placeholder="Extra vragen en info kan je hier kwijt...">{$txtCommentContent}</textarea></label>
                </div>
                <div id="submit">
                  <button id="btnCheckout" class="button is-medium is-info" name="action" value="checkout">Verder</button>
                  <button id="btnUpdate" class="button is-medium is-info is-inverted" name="action" value="updateAndBack">Verderwinkelen</button>
                </div>
              </div>
              {/option:oHasItems}
            </form>
          </div>
