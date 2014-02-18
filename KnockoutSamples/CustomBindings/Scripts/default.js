/// <reference path="lib/jquery-ui-1.10.4.js" />
/// <reference path="lib/jquery-2.1.0.intellisense.js" />
/// <reference path="lib/knockout-3.0.0.debug.js" />

ko.bindingHandlers.starRating = {
    init: function (element, valueAccessor) {
        $(element).addClass("starRating");
        for (var i = 0; i < 5; i++)
            $("<span>").appendTo(element);
        // Handle mouse events on the stars
        $("span", element).each(function (index) {
            $(this).hover(
                function () { $(this).prevAll().add(this).addClass("hoverChosen") },
                function () { $(this).prev().add(this).removeClass("hoverChosen") }
            ).click(function () {
                var observable = valueAccessor();
                observable(index + 1);
            });
        });
    },
    update: function (element, valueAccessor) {
        // Give the first x stars the "chosen" class, where x <= rating
        var observable = valueAccessor();
        $("span", element).each(function (index) {
            $(this).toggleClass("chosen", index < observable());
        });
    }
};

ko.bindingHandlers.fadeVisible = {
    init: function (element, valueAccessor) {
        // Start visible/invisible according to initial value
        var shouldDisplay = valueAccessor();
        $(element).toggle(shouldDisplay);
    },
    update: function (element, valueAccessor) {
        // On update, fade in/out
        var shouldDisplay = valueAccessor();
        shouldDisplay ? $(element).fadeIn() : $(element).fadeOut();
    }
};

ko.bindingHandlers.jqButton = {
    init: function (element) {
        $(":button");
    },
    update: function (element, valueAccessor) {
        var currentValue = valueAccessor();
        $(":button").add("option", "disabled", currentValue.enable === false);
    }
};

function Answer(text) { this.answerText = text; this.points = ko.observable(1); };

function SurveyViewModel(question, pointsBudget, answers) {
    this.question = question;
    this.pointsBudget = pointsBudget;
    this.answers = $.map(answers, function (text) { return new Answer(text) });
    this.save = function () { alert('To do') };

    this.pointsUsed = ko.computed(function () {
        var total = 0;
        for (var i = 0; i < this.answers.length; i++)
            total += this.answers[i].points();
        return total;
    }, this);
};

$(function () {
    ko.applyBindings(new SurveyViewModel("Which factors affect your technology choices?", 10, [
       "Functionality, compatibility, pricing - all that boring stuff",
       "How often it is mentioned on Hacker News",
       "Number of gradients/dropshadows on project homepage",
       "Totally believable testimonials on project homepage"
    ]))
});