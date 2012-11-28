
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;

using Cirrious.MvvmCross.Binding.Touch.ExtensionMethods;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Views;
using CrossUI.Touch.Dialog;
using Cirrious.MvvmCross.Dialog.Touch;
using Cirrious.MvvmCross.Touch.Interfaces;
using CrossUI.Touch.Dialog.Elements;


using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
    public class ShareView : MvxTouchDialogViewController<ShareViewModel>
    {
        public ShareView (MvxShowViewModelRequest request) : base (request, UITableViewStyle.Plain, new RootElement(), true)
        {
        }

        UIBarButtonItem buttonDone = new UIBarButtonItem(UIBarButtonSystemItem.Done);
        UIBarButtonItem buttonCancel;

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            buttonCancel = new UIBarButtonItem ("Cancel", UIBarButtonItemStyle.Bordered, (s, e) => {
                this.ViewModel.Cancel();
            });

            this.Title = "Share Wish List";
            this.NavigationItem.LeftBarButtonItem = buttonCancel;
            this.NavigationItem.RightBarButtonItem = buttonDone;

           

            this.buttonDone.Clicked += (s, e) => 
            {
                //Send email

                var to = this.ViewModel.GetEmailTo();
                var subject = this.ViewModel.GetEmailSubject();
                var body = this.ViewModel.GetEmailBody();

                var mailComposer = new MFMailComposeViewController();

                if (!string.IsNullOrEmpty(to))
                    mailComposer.SetToRecipients(to.Split(';'));

                mailComposer.SetSubject(subject);
                mailComposer.SetMessageBody(body, false);

                mailComposer.Finished += (s2, e2) => {
                    this.InvokeOnMainThread(() => 
                    {
                        this.DismissViewController(true, () => 
                        {
                            this.InvokeOnMainThread(() => this.NavigationController.PopViewControllerAnimated(true));
                        });
                    });
                };

                this.InvokeOnMainThread(() => 
                                        this.NavigationController.PresentViewController(mailComposer, true, () => { }));
            };

           
            this.Root = new RootElement("Share Wish List") { new Section("Choose Contacts to Share with") };
        }

        LoadingHUDView loadingView;

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            this.ViewModel.PropertyChanged += HandlePropertyChanged;

            this.ViewModel.LoadContacts ();
        }

        public override void ViewDidDisappear (bool animated)
        {
            this.ViewModel.PropertyChanged -= HandlePropertyChanged;
            base.ViewDidDisappear (animated);
        }

        void LoadTable()
        {
            this.Root.Clear();

            //Group contacts by name
            var gcs = from c in this.ViewModel.Contacts
                      group c by (c.DisplayName ?? "z").ToUpper().Substring(0, 1) into grouping
                      select grouping;


            //Populate the table with sections for each group and all items in each group
            foreach (var grp in gcs.OrderBy(g => g.Key))
            {
                var s = new Section(grp.Key);

                foreach (var c in grp)
                    s.Add(new SelectableContactElement(c));

                this.Root.Add(s);
            }
        }

        void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals ("IsLoading")) {
                if (this.ViewModel.IsLoading && loadingView == null) {
                    this.InvokeOnMainThread (() => {
                        loadingView = new LoadingHUDView ("Loading...", "");
                        this.TableView.AddSubview (loadingView);
                        loadingView.StartAnimating ();
                    });
                } else if (!this.ViewModel.IsLoading && this.loadingView != null) {
                    this.InvokeOnMainThread (() => {
                        loadingView.StopAnimating ();
                        loadingView.RemoveFromSuperview ();
                        loadingView = null;
                    });
                }
            } else if (e.PropertyName.Equals ("Contacts")) {

                this.BeginInvokeOnMainThread(() => {
                    LoadTable();
                    this.TableView.ReloadData();
                });
            }
        }
    }
}

