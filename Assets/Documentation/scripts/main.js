var PageLoader =
{
	callback: null,
	request: function(page, callback)
	{
		this.callback = callback;

		var head = document.getElementsByTagName('head')[0];
		var script = document.createElement('script');
		script.type = "text/javascript";
		script.src = page;
		head.appendChild(script);
	},
	response: function(content)
	{
		this.callback && this.callback(content);
	}
};

document.addEventListener("DOMContentLoaded", function(event)
{
	var _smallScreenMediaQuery = "(max-width: 44em)";

	var _bodyMenuOpenClass = "menu-open";
	var _menuSectionCollapsedClass = "collapsed";
	var _linkActiveClass = "active";
	var _loadingClass = "loading";
	var _searchingClass = "searching";
	var _linkNotFoundClass = "not-found";
	var _linkDisabledClass = "disabled";
	var _visibleClass = "visible";
	var _hljsClass = "hljs";

	var _title = "Composition Framework";
	var _rootUrl = window.location.href.substring(0, window.location.href.indexOf("Documentation.html"));
	var _defaultArticle = "overview/introduction.html";
	var _tableOfContentsUrl = "table-of-contents.html";

	var _onlineLink = document.getElementById("online-link");
	var _nextLink = document.getElementById("next-button");
	var _menuButton = document.getElementById("menu-button");
	var _menuOverlay = document.getElementById("menu-overlay");
	var _searchInput = document.getElementById("search-input");
	var _trademarkButton = document.getElementById("trademark-button");
	var _trademarkPopup = document.getElementById("trademark-popup");
	var _tableOfContents = document.getElementById("table-of-contents");
	var _titleText = document.getElementById("title-text");
	var _articleContent = document.getElementById("article-content");
	var _menuSections = document.getElementsByClassName("menu-section");
	var _searchRoots = document.getElementsByClassName("search-root");
	var _menuLinks = document.getElementsByClassName("menu-link");

	var _titleSeparator = "<i class='fas fa-chevron-right'></i>"

	var _currentVersion = "v10";
	var _currentArticle = "";
	
	var _searchIndex = null;
	var _searchTimeout = null;

	var _onlineUrl = "https://pirhosoft.com/projects/unity-composition/documentation/";

	var _nextArticle =
	{
		"tutorials/getting-started/setup.html": "tutorials/getting-started/game-logic.html",
		"tutorials/getting-started/game-logic.html": "tutorials/getting-started/interfaces.html",
		"tutorials/getting-started/interfaces.html": "tutorials/getting-started/transitions.html",
		"tutorials/getting-started/transitions.html": "tutorials/getting-started/data-binding.html",
		"topics/graphs/overview.html": "topics/graphs/nodes.html",
		"topics/graphs/nodes.html": "topics/graphs/control-flow.html",
		"topics/graphs/control-flow.html": "topics/graphs/instruction-store.html",
		"topics/graphs/instruction-store.html": "topics/graphs/debugging.html",
		"topics/graphs/debugging.html": "topics/graphs/running-from-script.html",
		"topics/graphs/running-from-script.html": "topics/graphs/custom-graphs.html",
		"topics/graphs/custom-graphs.html": "topics/graphs/custom-nodes.html",
		"topics/variables/overview.html": "topics/variables/creating-variables.html",
		"topics/variables/creating-variables.html": "topics/variables/defining-variables.html",
		"topics/variables/defining-variables.html": "topics/variables/accessing-variables.html",
		"topics/variables/accessing-variables.html": "topics/variables/writing-expressions.html",
		"topics/variables/writing-expressions.html": "topics/variables/custom-commands.html",
		"topics/variables/custom-commands.html": "topics/variables/exposing-variables.html",
		"topics/variables/exposing-variables.html": "topics/variables/custom-stores.html",
		"topics/bindings/overview.html": "topics/bindings/binding-roots.html",
		"topics/bindings/binding-roots.html": "topics/bindings/variable-bindings.html",
		"topics/bindings/variable-bindings.html": "topics/bindings/custom-binding-roots.html",
		"topics/bindings/custom-binding-roots.html": "topics/bindings/custom-variable-bindings.html",
		"topics/interface/overview.html": "topics/interface/controls.html",
		"topics/interface/controls.html": "topics/interface/messages.html",
		"topics/interface/messages.html": "topics/interface/menus-and-selections.html"
	}

	function IsSmallScreen()
	{
		return window.matchMedia && window.matchMedia(_smallScreenMediaQuery).matches;
	}

	function GetHash(version, article)
	{
		var name = article == _defaultArticle ? "" : article.substring(0, article.length - 5);
		return "#/" + version + "/" + name;
	}

	function ParseHash(hash)
	{
		if (!hash || hash == "#" || hash == "#/")
			hash = "#/" + _currentVersion + "/";

		var versionStart = 2;
		var versionEnd = hash.indexOf("/", versionStart);
		var articleStart = versionEnd + 1;

		var version = hash.substring(versionStart, versionEnd);
		var article = articleStart < hash.length ? (hash.substring(articleStart) + ".html") : _defaultArticle;

		return { version: version, article: article };
	}

	function GetSectionMenu(section)
	{
		return section.nextElementSibling;
	}

	function GetArticleUrl(link)
	{
		if (link.hash && link.href.startsWith(_rootUrl))
			return link.hash.substring(1);

		var href = link.href || link.parentElement.href;

		if (href && href.startsWith(_rootUrl) && href.endsWith(".html"))
			return href.substring(_rootUrl.length);

		return null;
	}

	function Initialize()
	{
		if (!IsSmallScreen())
			ShowMenu();

		_searchInput.oninput = DebounceSearch;
		_menuButton.onclick = ClickEvent(ToggleMenu);
		_menuOverlay.onclick = ClickEvent(HideMenu);
		_trademarkButton.onclick = ClickEvent(ShowTrademark);
		_trademarkPopup.onclick = HideTrademark;
		
		window.addEventListener("hashchange", SetPageFromHash);
		window.addEventListener("popstate", HistoryNavigation);
		window.addEventListener("click", LinkNavigation);

		_searchIndex = elasticlunr.Index.load(PageLoader.searchIndex);
		
		Load(_currentVersion + "/" + _tableOfContentsUrl, function(content)
		{
			SetTableOfContents(content);

			if (window.location.hash)
				SetPageFromHash();
			else
				LoadArticle(_defaultArticle, false);
		});
	}

	function ClickEvent(handler)
	{
		return function (event)
		{
			handler();
			event.preventDefault();
			event.stopPropagation();
		}
	}

	function DebounceSearch(event)
	{
		clearTimeout(_searchTimeout);

		_searchTimeout = setTimeout(function()
		{
			Search(event.target.value);
		}, 800);
	}

	function ToggleMenu()
	{
		if (HasClass(document.body, _bodyMenuOpenClass))
			HideMenu();
		else
			ShowMenu();
	}

	function ShowMenu()
	{
		AddClass(document.body, _bodyMenuOpenClass);
	}

	function HideMenu()
	{
		RemoveClass(document.body, _bodyMenuOpenClass);
	}

	function ShowTrademark()
	{
		AddClass(_trademarkPopup, _visibleClass);
	}
	
	function HideTrademark()
	{
		RemoveClass(_trademarkPopup, _visibleClass);
	}

	function SetHeight(element, height)
	{
		element.style.height = height ? height + "px" : height;
	}

	function ToggleSection(event)
	{
		// technique from Css Tricks: https://css-tricks.com/using-css-transitions-auto-dimensions/

		var wasCollapsed = HasClass(event.target, _menuSectionCollapsedClass);
		var menu = GetSectionMenu(event.target);

		ToggleClass(event.target, _menuSectionCollapsedClass, !wasCollapsed);

		if (menu)
		{
			if (wasCollapsed)
				ExpandSection(menu);
			else
				CollapseSection(menu);

			event.preventDefault();
			event.stopPropagation();
		}
	}

	function CollapseSection(menu)
	{
		var sectionHeight = menu.scrollHeight;
		var menuTransition = menu.style.transition;

		menu.style.transition = "";
		
		requestAnimationFrame(function()
		{
			SetHeight(menu, sectionHeight);
			menu.style.transition = menuTransition;

			requestAnimationFrame(function()
			{
				SetHeight(menu, 0);
			});
		});
	}

	function ExpandSection(menu)
	{
		SetHeight(menu, menu.scrollHeight);

		menu.addEventListener('transitionend', function()
		{
			menu.removeEventListener('transitionend', arguments.callee);
			SetHeight(menu, null);
		});
	}

	function HasClass(element, className)
	{
		return element.classList.contains(className);
	}

	function ToggleClass(element, className, condition)
	{
		if (condition)
			AddClass(element, className);
		else
			RemoveClass(element, className)
	}

	function AddClass(element, className)
	{
		element.classList.add(className);
	}

	function RemoveClass(element, className)
	{
		element.classList.remove(className);
	}

	function Search(query)
	{
		if (!query || query.length < 3)
		{
			RemoveClass(_tableOfContents, _searchingClass);
			
			for (var i = 0; i <_searchRoots.length; i++)
				delete _searchRoots[i].firstElementChild.dataset.searchCount;
		}
		else
		{
			AddClass(_tableOfContents, _searchingClass);
			var results = _searchIndex.search(query, {});

			var sections = {};

			for (var i = 0; i < _menuLinks.length; i++)
			{
				var link = _menuLinks[i];
				var found = IsInResults(link.href, results);

				if (found)
				{
					var section = link.closest(".search-root").firstElementChild;
					var count = sections[section.dataset.section] = sections[section.dataset.section] || { Count: 0 };
					count.Count++;
				}

				ToggleClass(link.parentElement, _linkNotFoundClass, !found);
			}
			
			for (var i = 0; i <_searchRoots.length; i++)
			{
				var root = _searchRoots[i];
				var section = root.firstElementChild;
				var count = sections[section.dataset.section];

				ToggleClass(root, _linkNotFoundClass, !count || count.Count == 0);
				section.dataset.searchCount = count ? count.Count : 0;
			}
		}
	}

	function IsInResults(href, results)
	{
		for (var i = 0; i < results.length; i++)
		{
			var result = results[i];
			if (href.endsWith(result.ref))
				return true;
		}

		return false;
	}

	function Load(url, onSuccess)
	{
		PageLoader.request(url, onSuccess);
	}

	function HistoryNavigation(event)
	{
		if (event.state)
			SetPage(event.state.version, event.state.article, false);
	}

	function LinkNavigation(event)
	{
		var url = GetArticleUrl(event.target);

		if (url)
		{
			LoadArticle(url, true);

			if (IsSmallScreen())
				HideMenu();

			event.preventDefault();
			event.stopPropagation();
		}
	}

	function SetPageFromHash()
	{
		var parsed = ParseHash(window.location.hash);
		SetPage(parsed.version, parsed.article, false);
	}

	function SetPage(version, article, pushState)
	{
		LoadArticle(article, pushState);
	}

	function SetTableOfContents(content)
	{
		_tableOfContents.innerHTML = content;

		for (var i = 0; i < _menuSections.length; i++)
		{
			var section = _menuSections[i];
			var menu = GetSectionMenu(section);

			section.onclick = ToggleSection;

			AddClass(section, _menuSectionCollapsedClass);

			if (menu)
				SetHeight(menu, 0);
		}
	}

	function LoadArticle(article, pushState)
	{
		_currentArticle = article;

		AddClass(_titleText, _loadingClass);
		AddClass(_articleContent, _loadingClass);

		var hash = GetHash(_currentVersion, _currentArticle);

		Load(_currentVersion + "/" + _currentArticle,
			function(content)
			{
				if (pushState)
					window.scrollTo(0, 0);

				SetArticleContent(content);
			},
			function()
			{
				LoadArticle(_defaultArticle, false);
			}
		);

		if (pushState)
			window.history.pushState({ version: _currentVersion, article: _currentArticle }, "", hash);

		window.history.replaceState({ version: _currentVersion, article: _currentArticle }, "", hash);
		
		_onlineLink.href = _onlineUrl + hash;
	}

	function SetArticleContent(content)
	{
		RemoveClass(_titleText, _loadingClass);
		RemoveClass(_articleContent, _loadingClass);

		var title = CleanUpName(_currentArticle);

		_titleText.innerHTML = title.replace(/ > /g, _titleSeparator);
		_articleContent.innerHTML = content;

		var codes = _articleContent.getElementsByClassName(_hljsClass);
		for (var i = 0; i < codes.length; i++)
			hljs.highlightBlock(codes[i]);

		document.title = _title + " " + title;

		UpdateNavigationLinks();

		var nextUrl = GetNextUrl(_currentArticle);
		ToggleClass(_nextLink, _linkDisabledClass, !nextUrl);
		_nextLink.href = nextUrl;
	}

	function CleanUpName(name)
	{
		return name
			.substring(0, name.length - 5)
			.replace(/\//g, " > ")
			.replace(/[-][0-9]+/g, "")
			.replace(/[0-9]-+/g, "")
			.replace(/^[a-z]/g, function (match) { return match.toUpperCase(); })
			.replace(/[- ]([a-z])/g, function (match, capture) { return " " + capture.toUpperCase(); });
	}

	function GetNextUrl(article)
	{
		return _nextArticle[article];
	}

	function UpdateNavigationLinks()
	{
		for (var i = 0; i < _menuSections.length; i++)
		{
			var section = _menuSections[i];
			RemoveClass(section, _linkActiveClass);
		}

		for (var i = 0; i < _menuLinks.length; i++)
		{
			var link = _menuLinks[i];
			var active = link.href.endsWith(_currentArticle);
			var parent = link.parentNode.parentNode.previousElementSibling;

			ToggleClass(link, _linkActiveClass, active);

			if (active && parent)
				AddClass(parent, _linkActiveClass)
		}
	}

	Initialize();
});