using System;
using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private decimal minimalFinalAmount = 500m;
        
        private static Dictionary<string, decimal> supportFeeDict= new Dictionary<string, decimal>
        {
            { "START", 250m },
            { "PRO", 400m },
            { "ENTERPRISE", 700m },
        };
        
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints) //tego nie możemy dotykać
        {
            validateData(customerId, seatCount, planCode, paymentMethod);
            
            string normalizedPlanCode = StandardNormalizer.normalize(planCode); 
            string normalizedPaymentMethod = StandardNormalizer.normalize(paymentMethod);

            //wcześniej te dwie metody nie były statyczne mimo, że operowały na statycznych danych
            var customer = CustomerRepository.GetById(customerId);
            var plan = SubscriptionPlanRepository.GetByCode(normalizedPlanCode);

            decimal baseAmount = plan.calculateBasePrice(seatCount);
            
            //discount calc
            IDiscount discountProcessor = new DiscountProcessor(customer,plan,baseAmount,seatCount,useLoyaltyPoints);
            decimal subtotalAfterDiscount = discountProcessor.processDiscount();

            string notes = discountProcessor.getNotes();
            
            decimal supportFee = 0m;
            if (includePremiumSupport)
            {
                supportFee = supportFeeDict.GetValueOrDefault(normalizedPlanCode);
                notes += "premium support included; ";
            }

            ITax taxProcessor = new TaxProcessor((subtotalAfterDiscount + supportFee), normalizedPaymentMethod, customer);

            taxProcessor.processTax();
            notes += taxProcessor.getNotes();
            
            decimal finalAmount = 
                //taxBase 
                subtotalAfterDiscount + supportFee + taxProcessor.getPaymentFee()
                + 
                //taxAmount
                taxProcessor.getTaxAmount()
                ;

            if (finalAmount < minimalFinalAmount)
            {
                finalAmount = minimalFinalAmount; //usunięcie magic numbera
                notes += "minimum invoice amount applied; ";
            }

            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = StandardRounder.round(baseAmount),
                DiscountAmount = StandardRounder.round(discountProcessor.getDiscountAmount()),
                SupportFee = StandardRounder.round(supportFee),
                PaymentFee = StandardRounder.round(taxProcessor.getPaymentFee()),
                TaxAmount = StandardRounder.round(taxProcessor.getTaxAmount()),
                FinalAmount = StandardRounder.round(finalAmount),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };

            IInvoiceSaver invoiceSaver = new BillingInvoiceSaverAdapter();
            invoiceSaver.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

                IEmailer emailer = new BillingEmailAdapter();
                emailer.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }

        public void validateData(int customerId, int seatCount, string planCode, string paymentMethod)
        {
            //sprawdzenie danych oddzielone do osobnej funkcji
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer id must be positive");
            }

            if (string.IsNullOrWhiteSpace(planCode))
            {
                throw new ArgumentException("Plan code is required");
            }

            if (seatCount <= 0)
            {
                throw new ArgumentException("Seat count must be positive");
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                throw new ArgumentException("Payment method is required");
            }
            
            if (!CustomerRepository.GetById(customerId).IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }
            
            if (!ITax.payTypeFeeDict.ContainsKey(StandardNormalizer.normalize(paymentMethod)))
            {
                throw new ArgumentException("Unsupported payment method");
            }
        }

    }
}
