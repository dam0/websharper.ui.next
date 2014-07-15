﻿// $begin{copyright}
//
// This file is confidential and proprietary.
//
// Copyright (c) IntelliFactory, 2004-2014.
//
// All rights reserved.  Reproduction or use in whole or in part is
// prohibited without the written consent of the copyright holder.
//-----------------------------------------------------------------
// $end{copyright}

namespace IntelliFactory.WebSharper.UI.Next

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.UI.Next

// This example is a translation of the basics from the AngularJS tutorial,
// found here: https://docs.angularjs.org/tutorial/.
// See this live at http://intellifactory.github.io/websharper.ui.next/#PhoneExample.fs !

[<JavaScript>]
module PhoneExample =

    // First, we declare types for phones and how to order them.

    type Phone = { Name: string; Snippet: string; Age: int }
    type Order = Alphabetical | Newest

    type Order with

        /// A textual representation of our orderings.
        static member Show order =
            match order with
            | Alphabetical -> "Alphabetical"
            | Newest -> "Newest"

    type Phone with

        /// A comparison function, based on whether we're sorting by name or age.
        static member Compare order p1 p2 =
            match order with
            | Alphabetical -> compare p1.Name p2.Name
            | Newest -> compare p1.Age p2.Age

        /// A filtering function.
        static member MatchesQuery q ph =
            ph.Name.Contains(q)
            || ph.Snippet.Contains(q)

    // This is our phones widget. We take a list of phones, and return
    // an document tree which can be rendered.
    let PhonesWidget (phones: list<Phone>) =
        // Firstly, we make a reactive variable for the list of phones.
        let allPhones = Var.Create phones
        // and one for the query string
        let query = Var.Create ""
        // And one for the ordering.
        let order = Var.Create Newest

        // The above vars are our model. Everything else is computed from them.
        // Now, compute visible phones under the current selection:
        let visiblePhones =
            View.Map2 (fun query order ->
                phones
                |> List.filter (Phone.MatchesQuery query)
                |> List.sortWith (Phone.Compare order))
                (View.FromVar query)
                (View.FromVar order)

        // A simple function for displaying the details of a phone:
        let showPhone ph =
            el "li" [
                el "span" [ txt ph.Name ]
                el "p" [ txt ph.Snippet ]
            ]

        let showPhones phones =
            Doc.Concat (List.map showPhone phones)

        // The main body.
        divc "row" [

            divc "col-sm-6" [

                // We specify a label, and an input box linked to our query RVar.
                txt "Search: "
                Doc.Input [Attr.Create "class" "form-control"] query

                // We then have a select box, linked to our orders variable
                txt "Sort by: "
                Doc.Select [Attr.Create "class" "form-control"] Order.Show [Newest; Alphabetical] order

            ]

            // Finally, we render the list of phones using RD.ForEach.
            // When the list changes, the DOM will be updated to reflect this.
            divc "col-sm-6" [
                el "ul" [ Doc.EmbedView (View.Map showPhones visiblePhones) ]
            ]
        ]

    let Main =
        // Here, we make a couple of phones, and declare a phonesWidget, then run the example.
        let defPhone name snip age =
            {
                Age = age
                Name = name
                Snippet = snip
            }
        PhonesWidget [
            defPhone "Nexus S" "Fast just got faster with Nexus S." 1
            defPhone "Motorola XOOM" "The Next, Next generation tablet" 2
            defPhone "Motorola XOOM with Wi-Fi" "The Next, Next generation tablet" 3
            defPhone "Samsung Galaxy" "The Ultimate Phone" 4
        ]

    let link txt href =
        Doc.Element "a" [(Attr.Create "href" href)] [Doc.TextNode txt]

    // Todo: I don't like this. There's got to be a nicer way of embedding links.
    let description =
        el "div" [
            Doc.TextNode "Taken from the "
            link "AngularJS Tutorial" "https://docs.angularjs.org/tutorial/"
            Doc.TextNode ", a list filtering and sorting application for phones."
        ]

    // Boilerplate..
    let Sample =
        Samples.Build()
            .Id("List Filtering and Sorting")
            .FileName(__SOURCE_FILE__)
            .Keywords(["todo"])
            .Render(Main)
            .RenderDescription(description)
            .Create()