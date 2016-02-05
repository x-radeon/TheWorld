// site.js
(function () {

    var ele = $("#username");
    ele.text("New Name");

    var main = $("#main");
    main.on("mouseenter", function () {
        this.style = "background-color: #898;";
    });

    main.on("mouseleave", function () {
        this.style = "";
    });

    var menuOptions = $("ul.menu li a");
    menuOptions.on("click", function () {
        var me = $(this);
        alert(me.text())
    });

})();