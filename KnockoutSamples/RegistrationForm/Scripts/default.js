/// <reference path="lib/jquery-2.1.0.intellisense.js" />
/// <reference path="lib/knockout-3.0.0.js" />

function PersonViewModel() {
    var self = this;
    self.details = ko.observable("");
    self.firstName = ko.observable("John");
    self.lastName = ko.observable("Smith");
    self.primaryPhone = ko.observable("");
    self.secondaryPhone = ko.observable("");
    self.annoyMe = ko.observable(true);
    self.annoyTimes = ko.observableArray(["In the morning", "In the afternoon", "In the evening"]);
    self.selectedTime = ko.observable("");
    self.products = ko.observableArray([
        { name: "Beer", price: 10.99 },
        { name: "Brats", price: 7.99 },
        { name: "Buns", price: 2.99 }
    ]);
    self.brats = { name: "Brats", price: 7.99 };
    self.favoriteProducts = ko.observableArray([brats]);
    self.phoneHasFocus = ko.observable(true);

    self.saveUserData = function (target, event) {
        self.jsonProducts = "";
        ko.utils.arrayForEach(self.products(), function (product) {
          self.jsonProducts = self.jsonProducts + product.name + " ";
        });
        alert(self.firstName() + " is trying to checkout!\n" +
            "Annoy me " + self.selectedTime() + "\n" +
            "Favorite product " + self.favoriteProducts().name + "\n" +
            "Product list: " + self.jsonProducts + "\n" +
            "JSON of Products: " + ko.toJSON(self.products));
        if (event.ctrlKey)
            alert("He was holding down the Control key for some reason.");
    }

    self.showDetails = function (target, event, details) {
        self.details(details);
    }

    self.hideDetails = function (target, event, details) {
        self.details(details);
    }

    self.loadPersonFromMVCController = function () {
        $.getJSON("UserData/GetPerson", loadMVCData);
    };

    function loadMVCData(data) {
        self.firstName(data[1].FirstName);
        self.lastName(data[1].LastName);
    };

    self.loadPersonFromWebApiController = function () {
        $.getJSON("http://localhost/HostDataWebAPI/api/UserData/GetPerson", loadWebApiData); 
    };

    function loadWebApiData(data) {
        self.firstName(data[0].FirstName);
        self.lastName(data[0].LastName);
    };
}

$(function () {
    var vm = PersonViewModel();
    ko.applyBindings(vm);
    console.log("Test log message");
});