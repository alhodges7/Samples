/// <reference path="lib/jquery-2.1.0.intellisense.js" />
/// <reference path="lib/knockout-3.0.0.debug.js" />

function Person(first, last) {
    var self = this;
    self.firstName = first,
    self.lastName = last;
}

function Product(name, price, tags, discount) {
    var self = this;
    self.name = ko.observable(name);
    self.price = ko.observable(price);
    tags = typeof (tags) !== "undefined" ? tags : [];
    self.tags = ko.observableArray(tags);
    discount = typeof (discount) !== "undefined" ? discount : 0;
    self.discount = ko.observable(discount);
    self.formattedDiscount = ko.computed(function () {
        return (self.discount() * 100) +"%";
    }, self);
}

function HelloViewModel() {
    var self = this;
    self.person = ko.observable(Person);
    self.personFullName = ko.computed(function () { return self.person().firstName + " " + self.person().lastName }, self);
    self.shoppingCart = ko.observableArray([
        new Product("Beer", 10.99, null, .20),
        new Product("Brats", 7.99, null, .10),
        new Product("Buns", 1.49, ["Baked goods", "Hot dogs"])
        ]);
    self.totalPrice = ko.computed(function () {
        self.total = 0;
        self.shoppingCart().forEach(function (product) {
            if (!product._destroy)
                self.total = self.total + product.price();
        });
        return self.total;
    });
    self.featuredProduct = ko.observable(new Product("Acme BBQ Sauce", 3.99));

    self.changeName = function () {
        if (self.person().firstName == "Bill") {
            self.person(new Person("Pete", "Adam"));
        }
        else {
            self.person(new Person("Bill", "Smith"));
        }
    };

    self.checkout = function () {
        alert("To Do");
    };

    self.addProduct = function () {
        this.shoppingCart.push(new Product("More Beer", 10.99, null, .20));
    };

    self.removeProduct = function (product) {
        self.shoppingCart.destroy(product);
    };
};

$(function () {
    var vm = new HelloViewModel();
    ko.applyBindings(vm);
    vm.person(new Person("Pete", "Adam"));
});
